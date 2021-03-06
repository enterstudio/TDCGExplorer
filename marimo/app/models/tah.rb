class Tah < ActiveRecord::Base
  belongs_to :arc
  acts_as_list :scope => :arc
  has_many :tsos, :dependent => :destroy, :order => "position"

  def collisions
    tsos.map(&:collisions).flatten.uniq.map(&:tah).uniq
  end

  def duplicates
    tsos.map(&:duplicates).flatten.uniq.map(&:tah).uniq
  end

  def rows
    tsos.map(&:row).compact.uniq
  end

  def row_names
    rows.map { |row| Tso.row_name(row) }
  end

  def row_caption
    row_names.join('/')
  end

  def col_zeros
    ary = tsos.map(&:col_zeros).flatten.uniq.map(&:tah).uniq
    ary.delete(self)
    ary
  end

  class Search
    attr_accessor :path

    def initialize(attributes)
      attributes.each do |name, value|
        send("#{name}=", value)
      end if attributes
    end

    def text=(text)
      self.path = text
    end

    def conditions
      @conditions ||= begin
        sql = "1"
        ret = [ sql ]
        unless path.blank?
          sql.concat " and path like ?"
          ret.push "%#{path}%"
        end
        ret
      end
    end

    def find_options
      { :conditions => conditions }
    end
  end
end
