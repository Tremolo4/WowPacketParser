using System;
using WowPacketParser.Enums;
using WowPacketParser.Misc;
using Guid=WowPacketParser.Misc.Guid;

namespace WowPacketParser.Parsing.Parsers
{
    public static class MailHandler
    {
        [Parser(Opcode.SMSG_RECEIVED_MAIL)]
        public static void HandleReceivedMail(Packet packet)
        {
            packet.ReadSingle("Time left"); // Sup with timers in float?
        }

        [Parser(Opcode.SMSG_SHOW_MAILBOX)]
        [Parser(Opcode.CMSG_GET_MAIL_LIST)]
        public static void HandleShowMailbox(Packet packet)
        {
            packet.ReadGuid("GUID");
        }

        [Parser(Opcode.CMSG_MAIL_TAKE_MONEY)]
        [Parser(Opcode.CMSG_MAIL_MARK_AS_READ)]
        [Parser(Opcode.CMSG_MAIL_CREATE_TEXT_ITEM)]
        public static void HandleTakeMoney(Packet packet)
        {
            packet.ReadGuid("Mailbox GUID");
            packet.ReadUInt32("Mail Id");
        }

        [Parser(Opcode.CMSG_MAIL_DELETE)]
        public static void HandleMailDelete(Packet packet)
        {
            packet.ReadGuid("Mailbox GUID");
            packet.ReadUInt32("Mail Id");
            packet.ReadUInt32("Template Id");
        }

        [Parser(Opcode.CMSG_MAIL_RETURN_TO_SENDER)]
        public static void HandleMailReturnToSender(Packet packet)
        {
            packet.ReadGuid("Mailbox GUID");
            packet.ReadUInt32("Mail Id");
            packet.ReadGuid("Sender GUID");
        }

        [Parser(Opcode.SMSG_MAIL_LIST_RESULT)]
        public static void HandleMailListResult(Packet packet)
        {
            packet.ReadUInt32("Total Mails");
            var count = packet.ReadByte("Shown Mails");
            for (var i = 0; i < count; ++i)
            {
                packet.ReadUInt16("Message Size", i);
                packet.ReadUInt32("Mail Id", i);
                var mailType = packet.ReadEnum<MailType>("Message Type", TypeCode.Byte, i);
                switch (mailType) // Read GUID if MailType.Normal, int32 (entry) if not
                {
                    case MailType.Normal:
                        packet.ReadGuid("Player GUID", i);
                        break;
                    case MailType.Creature:
                        Console.WriteLine("[" + i + "] Entry: " + StoreGetters.GetExistingDatabaseString(StoreNameType.Unit, packet.ReadInt32()));
                        break;
                    case MailType.GameObject:
                        Console.WriteLine("[" + i + "] Entry: " + StoreGetters.GetExistingDatabaseString(StoreNameType.GameObject, packet.ReadInt32()));
                        break;
                    case MailType.Item:
                        Console.WriteLine("[" + i + "] Entry: " + StoreGetters.GetExistingDatabaseString(StoreNameType.Item, packet.ReadInt32()));
                        break;
                    default:
                        packet.ReadInt32("Entry", i);
                        break;
                }
                packet.ReadUInt32("COD", i);
                packet.ReadUInt32("Unk uint32", i);
                packet.ReadUInt32("Stationery", i);
                packet.ReadUInt32("Money", i);
                packet.ReadUInt32("Flags", i);
                packet.ReadSingle("Time?", i);
                packet.ReadUInt32("Template Id", i);
                packet.ReadCString("Subject", i);
                packet.ReadCString("Body", i);
                var items = packet.ReadByte("Item Count", i);
                for (var j = 0; j < items; ++j)
                {
                    packet.ReadByte("Item Index", i, j);
                    packet.ReadUInt32("Item GuidLow", i, j);
                    packet.ReadEntryWithName<UInt32>(StoreNameType.Item, "Item Id", i, j);
                    for (var k = 0; k < 7; ++k)
                    {
                        packet.ReadUInt32("Item Enchantment Id", i, j, k);
                        packet.ReadUInt32("Item Enchantment Duration", i, j, k);
                        packet.ReadUInt32("Item Enchantment Charges", i, j, k);
                    }
                    packet.ReadInt32("Item Random Property Id", i, j);
                    packet.ReadUInt32("Item Suffix Factor", i, j);
                    packet.ReadUInt32("Item Count", i, j);
                    packet.ReadUInt32("Item SpellCharges", i, j);
                    packet.ReadUInt32("Item Max Durability", i, j);
                    packet.ReadUInt32("Item Durability", i, j);
                    packet.ReadByte("Unk byte", i, j);
                }
            }
        }

        [Parser(Opcode.MSG_QUERY_NEXT_MAIL_TIME)]
        public static void HandleNullMail(Packet packet)
        {
            if (packet.Direction == Direction.ClientToServer)
                return;

            // Math.Abs(this float) > 0.0f returns whether the player has received a new mail since last visiting a mailbox
            packet.ReadSingle("Time since last time visiting a mailbox (can be < 0.0)");

            var count = packet.ReadUInt32("Count");
            for (var i = 0; i < count; ++i)
            {
                var data = packet.ReadUInt64();
                if (data == 0 || ((data & 0xFFFFFFFF00000000) >> 32) == 0)
                    Console.WriteLine("Entry: " + ((data & 0x00000000FFFFFFFF) >> 32));
                else
                {
                    var guid = new Guid(data);
                    Console.WriteLine("[" + i + "] GUID: " + guid);
                }
                packet.ReadUInt32("COD", i);
                packet.ReadUInt32("Unk uint32", i);
                packet.ReadUInt32("Stationery", i);
                packet.ReadSingle("Time?", i);
            }
        }

        [Parser(Opcode.SMSG_SEND_MAIL_RESULT)]
        public static void HandleSendMailResult(Packet packet)
        {
            packet.ReadUInt32("Mail Id");
            var action = packet.ReadEnum<MailActionType>("Mail Action", TypeCode.UInt32);
            var error = packet.ReadEnum<MailErrorType>("Mail Error", TypeCode.UInt32);
            if (error == MailErrorType.Equip)
                packet.ReadUInt32("Equip Error");
            else if (action == MailActionType.ItemTaken)
            {
                packet.ReadUInt32("Item Low GUID");
                packet.ReadUInt32("Item count");
            }
        }

        [Parser(Opcode.CMSG_SEND_MAIL)]
        public static void HandleSendMail(Packet packet)
        {
            packet.ReadGuid("Mailbox GUID");
            packet.ReadCString("Receiver");
            packet.ReadCString("Subject");
            packet.ReadCString("Body");
            packet.ReadUInt32("Stationery?");
            packet.ReadUInt32("Unk Uint32");
            var items = packet.ReadByte("Item Count");
            for (var i = 0; i < items; ++i)
            {
                packet.ReadByte("Slot", i);
                packet.ReadGuid("Item GUID", i);
            }
            packet.ReadUInt32("Money");
            packet.ReadUInt32("COD");
            packet.ReadUInt64("Unk Uint64");
            packet.ReadByte("Unk Byte");

        }

        [Parser(Opcode.CMSG_MAIL_TAKE_ITEM)]
        public static void HandleMailTakeItem(Packet packet)
        {
            packet.ReadGuid("Mailbox GUID");
            packet.ReadUInt32("Mail Id");
            packet.ReadUInt32("Item Low GUID");
        }

        //CMSG_MAELSTROM_GM_SENT_MAIL
    }
}
